using UnityEngine;
using System.Collections;

public class Block {
	public int type;
	public bool vis;
	public GameObject block;

	public Block(int t, bool v) {
		type = t;
		vis = v;
	}
}

public class GenerateLandscape : MonoBehaviour {

	public static int width = 128;
	public static int depth = 128;
	public static int height = 128;
	public int heightScale = 20;
	public int heightOffset = 100;
	public float detailScale = 25.0f;

	public GameObject grassBlock;
	public GameObject sandBlock;
	public GameObject snowBlock;
	public GameObject diamondBlock;
	public GameObject cloudBlock;

	Block[,,] worldBlocks = new Block[width, height, depth];

	// Use this for initialization
	void Start () {
		int seed = 576620;//(int)Network.time * 10;
		Debug.Log (seed);
		for (int z = 0; z < depth; z++) {
			for (int x = 0; x < width; x++) {
				int y = (int)(Mathf.PerlinNoise (x / detailScale, (z + seed) / detailScale) * heightScale) + heightOffset;
				Vector3 blockPos = new Vector3 (x, y, z);

				CreateBlock (y, blockPos, true);

				while (y > 0) {
					y--;
					blockPos = new Vector3 (x, y, z);
					CreateBlock (y, blockPos, false);
				}

			}
		}

		FixHoles ();

		DrawCloud (20, 100);

		DigMines (5, 500);
	}

	void CreateBlock(int y, Vector3 blockPos, bool create) {
		if (blockPos.x < 0 || blockPos.x >= width || blockPos.y < 0 || blockPos.y >= height || blockPos.z < 0 || blockPos.z >= depth)
			return;
		if (y > 15 + heightOffset) {
			Block block = new Block (1, create);
			if (create)
				block.block = (GameObject)Instantiate (snowBlock, blockPos, Quaternion.identity);
			worldBlocks [(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = block;
		} else if (y > 5 + heightOffset) {
			Block block = new Block (2, create);
			if (create)
				block.block = (GameObject)Instantiate (grassBlock, blockPos, Quaternion.identity);
			worldBlocks [(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = block;
		} else {
			Block block = new Block (3, create);
			if (create)
				block.block = (GameObject)Instantiate (sandBlock, blockPos, Quaternion.identity);
			worldBlocks [(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = block;
		}

		if (y > 80 && y < 90 && Random.Range(0, 100) < 30) {
			Block block = new Block (5, create);
			if (create)
				block.block = (GameObject)Instantiate (diamondBlock, blockPos, Quaternion.identity);
			worldBlocks [(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = block;
		}
	}

	void DrawBlock(Vector3 blockPos) {
		if (blockPos.x < 0 || blockPos.x >= width || blockPos.y < 0 || blockPos.y >= height || blockPos.z < 0 || blockPos.z >= depth)
			return;
		
		Block b = worldBlocks [(int)blockPos.x, (int)blockPos.y, (int)blockPos.z];
		if (b != null) {
			//Debug.LogFormat ("{0} -- {1}", (int)blockPos.y, b.type);
			if (!b.vis) {
				b.vis = true;
				if (b.type == 1) {
					b.block = (GameObject)Instantiate (snowBlock, blockPos, Quaternion.identity);
				} else if (b.type == 2) {
					b.block = (GameObject)Instantiate (grassBlock, blockPos, Quaternion.identity);
				} else if (b.type == 3) {
					b.block = (GameObject)Instantiate (sandBlock, blockPos, Quaternion.identity);
				} else if (b.type == 5) {
					b.block = (GameObject)Instantiate (diamondBlock, blockPos, Quaternion.identity);
				} else {
					worldBlocks [(int)blockPos.x, (int)blockPos.y, (int)blockPos.z].vis = false;
					Destroy (b.block);
					worldBlocks [(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = null;
				}
			}
		}
	}

	void FixHoles() {
		for (int x = 0; x < width; x++) {
			for (int z = 0; z < depth; z++) {
				for (int y = height - 2; y >= 1; y--) {
					Block b = worldBlocks [x, y, z];
					Block upBlock = worldBlocks [x, y + 1, z];
					if (upBlock != null && upBlock.vis) {
						for (int i = -1; i <= 1; i++) {
							int k = 0;
							if (i == 0 || x <= 0 || x >= width - 1 || z <= 0 || z >= depth - 1)
								continue;
							Block neighbourDown = worldBlocks [x + i, y - 1, z + k];
							Block neighbour = worldBlocks [x + i, y, z + k];
							if ((neighbour == null || !neighbour.vis) && neighbourDown != null && neighbourDown.vis) {
								b.vis = true;
								b.type = 4;
								Vector3 blockPos = new Vector3 (x, y, z);
								CreateBlock (y, blockPos, true);
								break;
							}
						}
						for (int k = -1; k <= 1; k++) {
							int i = 0;
							if (k == 0 || x <= 0 || x >= width - 1 || z <= 0 || z >= depth - 1)
								continue;
							Block neighbourDown = worldBlocks [x + i, y - 1, z + k];
							Block neighbour = worldBlocks [x + i, y, z + k];
							if ((neighbour == null || !neighbour.vis) && neighbourDown != null && neighbourDown.vis) {
								b.vis = true;
								b.type = 4;
								Vector3 blockPos = new Vector3 (x, y, z);
								CreateBlock (y, blockPos, true);
								break;
							}
						}
					}
				}
			}
		}
	}

	void DigMines (int numMines, int mSize) {
		int holeSize = 2;
		for (int i = 0; i < numMines; i++) {
			int xpos = Random.Range (10, width - 10);
			int ypos = Random.Range (10, height - 10);
			int zpos = Random.Range (10, depth - 10);

			for (int j = 0; j < mSize; j++) {
				for (int x = -holeSize; x <= holeSize; x++) {
					for (int y = -holeSize; y <= holeSize; y++) {
						for (int z = -holeSize; z <= holeSize; z++) {
							if (!(x == 0 && y == 0 && z == 0)) {
								Vector3 blockPos = new Vector3 (xpos + x, ypos + y, zpos + z);
								//Debug.Log (blockPos);
								Block b = worldBlocks [(int)blockPos.x, (int)blockPos.y, (int)blockPos.z];
								if (b != null && b.block != null) {
									Destroy (b.block);
								}
								worldBlocks [(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = null;
							}
						}
					}
				}

				xpos += Random.Range (-1, 2);
				zpos += Random.Range (-1, 2);
				ypos += Random.Range (-1, 2);
				//Debug.LogFormat ("({0}, {1}, {2})", xpos, ypos, zpos);
				if (xpos < holeSize || xpos >= width - holeSize) xpos = width/2;
				if (ypos < holeSize || ypos >= height - holeSize)
					ypos = height / 2;
				if (zpos < holeSize || zpos >= depth - holeSize)
					zpos = depth / 2;
			}
		}

		for (int z = 1; z < depth - 1; z++) {
			for (int x = 1; x < depth - 1; x++) {
				for (int y = 1; y < height - 1; y++) {
					if (worldBlocks [x, y, z] == null) {
						for (int x1 = -1; x1 <= 1; x1++) {
							for (int y1 = -1; y1 <= 1; y1++) {
								for (int z1 = -1; z1 <= 1; z1++) {
									if (!(x1 == 0 && y1 == 0 && z1 == 0)) {
										Vector3 neighbour = new Vector3 (x + x1, y + y1, z + z1);
										DrawBlock (neighbour);
									}
								}
							}
						}
					}
				}
			}
		}
	}

	void DrawCloud (int numClouds, int cSize) {
		for (int i = 0; i < numClouds; i++) {
			int xpos = Random.Range (0, width);
			int zpos = Random.Range (0, depth);
			for (int j = 0; j < cSize; j++) {
				Vector3 blockPos = new Vector3 (xpos, height - 1, zpos);
				Instantiate (cloudBlock, blockPos, Quaternion.identity);
				worldBlocks [(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block (4, true);
				xpos += Random.Range (-1, 2);
				zpos += Random.Range (-1, 2);
				if (xpos < 0 || xpos >= width)
					xpos = width / 2;
				if (zpos < 0 || zpos >= depth)
					zpos = depth / 2;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (new Vector3 (Screen.width / 2.0f, Screen.height / 2.0f, 0));
			if (Physics.Raycast (ray, out hit, 100.0f)) {
				if (!hit.transform.gameObject.CompareTag ("Block"))
					return;

				Vector3 blockPos = hit.transform.position;

				if ((int)blockPos.y == 0)
					return;

				worldBlocks [(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = null;

				Destroy (hit.transform.gameObject);

				for (int x = -1; x <= 1; x++) {
					for (int y = -1; y <= 1; y++) {
						for (int z = -1; z <= 1; z++) {
							if (!(x == 0 && y == 0 && z == 0)) {
								Vector3 neighbour = new Vector3 (blockPos.x + x, blockPos.y + y, blockPos.z + z);
								DrawBlock (neighbour);
							}
						}
					}
				}
			}
		} else if (Input.GetMouseButtonDown (1)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (new Vector3 (Screen.width / 2, Screen.height / 2, 0));
			if (Physics.Raycast (ray, out hit, 1000.0f)) {
				Vector3 blockPos = hit.transform.position;
				Vector3 newBlockPos = blockPos + hit.normal;

				CreateBlock ((int)newBlockPos.y, newBlockPos, true);

				Block b = worldBlocks [(int)blockPos.x, (int)blockPos.y, (int)blockPos.z];
				if (b != null && b.vis) {
					bool vis = false;
					for (int x = -1; x <= 1; x++) {
						for (int y = -1; y <= 1; y++) {
							for (int z = -1; z <= 1; z++) {
								Vector3 neighbour = blockPos + new Vector3 (x, y, z);
								if (Vector3.Distance(blockPos, neighbour) == 1) {
									if (neighbour.x < 0 || neighbour.x >= width || neighbour.y < 0 || neighbour.y >= height || neighbour.z < 0 || neighbour.z >= depth)
										continue;
									Block neighbourBlock = worldBlocks [(int)neighbour.x, (int)neighbour.y, (int)neighbour.z];
									if (neighbourBlock == null || !neighbourBlock.vis) {
										vis = true;
										break;
									}
								}
							}
							if (vis)
								break;
						}
						if (vis)
							break;
					}
					if (!vis) {
						b.vis = false;
						Destroy (b.block);
						b.block = null;
					}
				}
			}
		}
	}
}
